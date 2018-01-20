<?php 

//
//  SyncUserPrompts.php
//  Happy31.iOSApp
//
//  Copyright © 2017 Denis Klyucherov. All rights reserved.
//
// Sync user's prompts
// INPUT: user_prompt_id, user_id, prompt_id, created_at, is_done, is_sync (True/False/sync_all)
// OUTPUT: user_prompt_id, user_id, is_sync (True/False)

require("database.php");
require("mysqldao.php");

$jsondata = "php://input";

// Get content of posted JSON String
$phpjsonstring = file_get_contents( $jsondata);

// Decoding content of posted JSON String
$data = json_decode($phpjsonstring, true);

// Creating instance of MySQLDao class
$dao = new MySQLDao();

$dao->openConnection();

// Temp array with results of inserting rows
$tmp_array = array();

$returnValue = array();

// If server received in Json command "sync_all", send table "users_prompts"
if ($data[0]["is_sync"] == "sync_all")
{
	// Getting table users_prompts for the current user
	$tableUsersPrompts = $dao->getTableUsersPrompts($data[0]["user_id"]);
	
	if(!empty($tableUsersPrompts))
		while ($res = mysqli_fetch_assoc($tableUsersPrompts))
			$returnValue[] = $res; // If there are some data on server, return them
	
	else // There are no data on server to sync
	{
		$tmp_array["user_prompt_id"] = 0;
		$tmp_array["user_id"] = $data[0]["user_id"];

		$returnValue[] = $tmp_array;
	}

	echo json_encode($returnValue);

	if (json_last_error() !== JSON_ERROR_NONE)
	echo json_last_error_msg();

	$dao->closeConnection();

	return;
}

// If server received in Json command array of user's prompts, insert them and send back status
// Saving current prompt into users_prompt table and return user_prompt_id, user_id and is_sync status
foreach ($data as $value)
{
	// Validation, if fields are not empty 
	if(!empty($value["user_id"]) && !empty($value["prompt_id"]) && !empty($value["created_at"]) && !empty($value["is_done"]) && !empty($value["is_sync"]))
	{
		$tmp_array["user_prompt_id"] = $value["user_prompt_id"];
		$tmp_array["user_id"] = $value["user_id"];

		// Check if inserted user prompt is already exists in the table "users_prompts"
		$isInserted = $dao->checkIfCurrentUserPromptExists($value["user_id"], $value["prompt_id"], $value["created_at"], $value["is_done"]);

		if ($isInserted === "True") // If user prompts already exists, update it (coluld be different IsDone status) and send True to the client
		{
			$isUpdated = $dao->updateIsDoneStatus($value["user_id"], $value["prompt_id"], $value["created_at"], $value["is_done"]);
			$tmp_array["is_sync"] = $isUpdated;
		}
		else
		{
			$saveStatus = $dao->saveCurrentUserPrompt($value["user_id"], $value["prompt_id"], $value["created_at"], $value["is_done"]);
			$tmp_array["is_sync"] = $saveStatus; // Return true/false depending on if inserting was successful or not
		}

		$returnValue[] = $tmp_array;
	}
	else
	{
		$returnValue["status"] = "error";
		$returnValue["message"] = "Missing required field";
		echo json_encode($result);
		return;
	}
}

echo json_encode($returnValue);

if (json_last_error() !== JSON_ERROR_NONE)
	echo json_last_error_msg();

$dao->closeConnection();

?>