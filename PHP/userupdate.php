<?php 

//
//  UserUpdate.php
//  Happy31.iOSApp
//
//  Copyright © 2017 Denis Klyucherov. All rights reserved.
//
// Update user
// INPUT: user_id, first_name, last_name, email, avatar
// OUTPUT: status, message

require("database.php");
require("mysqldao.php");

$jsondata = "php://input";

// Get content of posted JSON String
$phpjsonstring = file_get_contents( $jsondata);

// Decoding content of posted JSON String
$data = json_decode($phpjsonstring, true);

$user_id = $data["user_id"];
$first_name = $data["first_name"];
$last_name = $data["last_name"];
$email = $data["email"];
$password = $data["password"];
$avatar = $data["avatar"];

$returnValue = array();

// Validation, if fields are not empty (Validation also provides on client side)
if(empty($user_id) || empty($first_name) || empty($last_name) || empty($email))
{
	$returnValue["status"] = "error";
	$returnValue["message"] = "Missing required field";
	echo json_encode($returnValue);
	return;
}

// Creating instance of MySQLDao class
$dao = new MySQLDao();

$dao->openConnection();

// Get user email from database to compare it with the new email wnen update
$userOldEmail = $dao->getUserEmailToUpdate($user_id);

// If it's a new email, check if the new email already exists in the database
if ($userOldEmail != $email)
{
	$userDetails = $dao->getUserRegisterData($email);
	if (!empty($userDetails)) // If the new email already exists
	{
		$returnValue["status"] = "Error";
		$returnValue["message"] = "New email already exists! Please, enter another one";

		echo json_encode($returnValue);
		return;
	}
}

// Hashing user password if it's not empty
if (!empty($password))
	$password = password_hash($password, PASSWORD_BCRYPT); 

// Updating user by id
$isUpdated = $dao->updateUser($user_id, $first_name, $last_name, $email, $password, $avatar);

if ($isUpdated == "True")
{
	$returnValue["status"] = "Success";
	$returnValue["message"] = "Profile has been successfully updated";
}
else
{
	$returnValue["status"] = "Error";
	$returnValue["message"] = "There was an error when updating or user has not been found";
}

echo json_encode($returnValue);

if (json_last_error() !== JSON_ERROR_NONE)
	echo json_last_error_msg();

$dao->closeConnection();

?>