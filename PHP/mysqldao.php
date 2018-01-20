<?php

//
//  MySQLDAO.php
//  Happy31.iOSApp
//
//  Copyright © 2017 Denis Klyucherov. All rights reserved.
//
//
// MySQL Data access object

class MySQLDao {
	var $dbhost = null;
	var $dbuser = null;
	var $dbpass = null;
	var $conn = null;
	var $dbname = null;
	var $result = null;

function __construct() {
	$this->dbhost = Database::$dbhost;
	$this->dbuser = Database::$dbuser;
	$this->dbpass = Database::$dbpass;
	$this->dbname = Database::$dbname;
}

public function openConnection() {
	$this->conn = new mysqli($this->dbhost, $this->dbuser, $this->dbpass, $this->dbname);
	if (mysqli_connect_errno())
		echo new Exception("Could not establish connection with database");
}

public function getConnection() {
	return $this->conn;
}

public function closeConnection() {
	if ($this->conn != null)
		$this->conn->close();
}

// Getting data for registration process (id and provider) by email
public function getUserRegisterData($email)
{
	$returnValue = array();
	$sql = "SELECT id, provider, created_at FROM users WHERE email='" . $email . "'";

	$result = $this->conn->query($sql);
	if ($result != null && (mysqli_num_rows($result) > 0)) {
		$row = $result->fetch_array(MYSQLI_ASSOC);
	
		if (!empty($row)) {
			$returnValue = $row;
		}
	}
	return $returnValue;
}

// Getting data for login process (id, password and provider) by email
public function getUserLoginData($email)
{
	$returnValue = array();
	$sql = "SELECT id, first_name, last_name, password, provider, avatar, created_at FROM users WHERE email='" . $email . "'";

	$result = $this->conn->query($sql);
	if ($result != null && (mysqli_num_rows($result) > 0)) {
		$row = $result->fetch_array(MYSQLI_ASSOC);
		
		if (!empty($row)) {
			$returnValue = $row;
		}
	}
	return $returnValue;
}

// Getting user email by its id to check if it matchs with the new email when update
public function getUserEmailToUpdate($id)
{
	$returnValue = array();
	$sql = "SELECT email FROM users WHERE id='" . $id . "'";

	$result = $this->conn->query($sql);
	if ($result != null && (mysqli_num_rows($result) > 0)) {
		$row = $result->fetch_array(MYSQLI_ASSOC);
		
		if (!empty($row)) {
			$returnValue = $row;
		}
	}
	return $returnValue["email"];
}

// Registering user
public function registerUser($id, $first_name, $last_name, $email, $password, $provider)
{
	$sql = "INSERT INTO users (id, first_name, last_name, email, password, provider) VALUES (?, ?, ?, ?, ?, ?)";
	
	$statement = $this->conn->prepare($sql); // Prepare an SQL statement for execution

	if (!$statement)
		throw new Exception($statement->error);

	$statement->bind_param("ssssss", $id, $first_name, $last_name, $email, $password, $provider);
	$returnValue = $statement->execute();

	$statement->close();

	return $returnValue;
}

// Update user
public function updateUser($id, $first_name, $last_name, $email, $password, $avatar)
{
	if (!empty($password))
		$password = ", password = '" . $password . "'";

	$sql = "UPDATE users SET first_name = '" . $first_name . "', last_name = '" . $last_name . "', email = '" . $email . "'" . $password . ", avatar = '" . $avatar . "' WHERE id = '" . $id . "'";
	
	$statement = $this->conn->query($sql);

	if (!$statement)
		throw new Exception($statement->error);

	$returnValue = $this->conn->affected_rows; // Return count of affected rows

	if ($returnValue >= 0)
		return "True";
	else
		return "False";
}

// ========= DO NOT USE ==============
// Getting random prompt from each category by user_id
public function getCurrentUserPrompt($user_id)
{
	$returnValue = array();
	
	// Creating temporary table to keep there prompts id which haven't been showed to user for the last 24 hours (from different categories)
	$tmp_table = str_replace("-", "_", $user_id);
	$sql = "CREATE TEMPORARY TABLE IF NOT EXISTS tmp_" . $tmp_table . "(id int) ENGINE=MEMORY AS 
 				SELECT id FROM prompts WHERE category NOT IN
 					(SELECT category from prompts AS t1, users_prompts AS t2 WHERE t1.id = t2.prompt_id AND TIMESTAMPDIFF(MINUTE, created_at, NOW()) < 330 AND t2.user_id = '" . $user_id . "')";
	$statement = $this->conn->prepare($sql);
	$statement->execute();
	
	// Randomly choosing one of the prompt id
	$sql = "SELECT * FROM tmp_" . $tmp_table . "
 				ORDER BY RAND()
 				LIMIT 1";
	$result = $this->conn->query($sql);
	
	// Removing temporary table
	$sql = "DROP TABLE IF EXISTS tmp_" . $tmp_table;
	$statement = $this->conn->prepare($sql);
	$statement->execute();

	if ($result != null && (mysqli_num_rows($result) > 0)) {
		$row = $result->fetch_array(MYSQLI_ASSOC);
		
		if (!empty($row)) {
			$returnValue = $row;
		}
	}
	return $returnValue;
}

// Check if inserted user prompt is already exists in the table "users_prompts"
public function checkIfCurrentUserPromptExists($user_id, $prompt_id, $created_at)
{
	$sql = "SELECT * FROM users_prompts WHERE user_id = '" . $user_id . "' AND prompt_id = " . $prompt_id . " AND created_at = '" . $created_at . "' LIMIT 1";

	$statement = $this->conn->query($sql);

	if (!$statement)
		throw new Exception($statement->error);

	$returnValue = $this->conn->affected_rows; // Return count of affected rows

	if ($returnValue > 0)
		return "True";
	else
		return "False";
}

// Update IsDone status of users_prompts table
public function updateIsDoneStatus($user_id, $prompt_id, $created_at, $is_done)
{
	$sql = "UPDATE users_prompts SET is_done = '" . $is_done . "' WHERE user_id = '" . $user_id . "' AND prompt_id = " . $prompt_id . " AND created_at = '" . $created_at . "'";
	
	$statement = $this->conn->query($sql);

	if (!$statement)
		throw new Exception($statement->error);

	$returnValue = $this->conn->affected_rows; // Return count of affected rows

	if ($returnValue >= 0)
		return "True";
	else
		return "False";
}

// Saving user's prompts into users_prompt table if there is not any dublicate
public function saveCurrentUserPrompt($user_id, $prompt_id, $created_at, $is_done)
{
	$sql = "INSERT INTO users_prompts (user_id, prompt_id, created_at, is_done) 
				SELECT * FROM (SELECT '" . $user_id . "', " . $prompt_id . ", '" . $created_at . "', '" . $is_done . "') AS tmp WHERE NOT EXISTS (SELECT * FROM users_prompts WHERE user_id = '" . $user_id . "' AND prompt_id = " . $prompt_id . " AND created_at = '" . $created_at . "') LIMIT 1";

	$statement = $this->conn->query($sql);

	if (!$statement)
		throw new Exception($statement->error);

	$returnValue = $this->conn->affected_rows; // Return count of affected rows

	if ($returnValue > 0)
		return "True";
	else
		return "False";
}

// Getting table users_prompts
public function getTableUsersPrompts($user_id)
{
  	$returnValue = array();
	$sql = "SELECT * FROM users_prompts where user_id = '" . $user_id . "'";
	$result = $this->conn->query($sql);

	if ($result != null && (mysqli_num_rows($result) > 0)) {
			$returnValue = $result;
	}
	return $returnValue;
}

// Getting table Prompts
public function getTablePrompts()
{  
  	$returnValue = array();
	$sql = "SELECT * FROM prompts";
	$result = $this->conn->query($sql);

	if ($result != null && (mysqli_num_rows($result) > 0)) {
			$returnValue = $result;
	}
	return $returnValue;
}

}
?>