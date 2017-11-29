<?php 

// Sync user's prompts
// INPUT: user_prompt_id, user_id, prompt_id, created_at, is_sync (True/False/sync_all)
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

// If server received in Json command "sync_all", send table "users_prompts"
if ($data[0]["is_sync"] == "sync_all")
{
	// Getting table users_prompts for the current user
	$tableUsersPrompts = $dao->getTableUsersPrompts($data[0]["user_id"]);
	$returnValue = array();

	if(!empty($tableUsersPrompts))
		while ($res = mysqli_fetch_assoc($tableUsersPrompts))
			$returnValue[] = $res; // If there are some data on server, return them
	
	else // There are no data on server to sync
	{
		$returnValue["user_prompt_id"] = "";
		$returnValue["user_id"] = $data[0]["user_id"];
		$returnValue["prompt_id"] = "";
		$returnValue["created_at"] = "";
		$returnValue["is_sync"] = "False";
	}

	echo json_encode($returnValue);

	if (json_last_error() !== JSON_ERROR_NONE)
	echo json_last_error_msg();

	$dao->closeConnection();

	return;
}

// If server received in Json command array of user's prompts, insert them and send back status

// Temp array with results of inserting rows
$tmp_array = array();

// Saving current prompt into users_prompt table and return user_prompt_id, user_id and is_sync status
foreach ($data as $value)
{
	// Validation, if fields are not empty 
	if(!empty($value["user_id"]) && !empty($value["prompt_id"]) && !empty($value["created_at"]) && !empty($value["is_sync"]))
	{
		$tmp_array["user_prompt_id"] = $value["user_prompt_id"];
		$tmp_array["user_id"] = $value["user_id"];

		// Check if inserted user prompt is already exists in the table "users_prompts"
		$isInserted = $dao->checkIfCurrentUserPromptExists($value["user_id"], $value["prompt_id"], $value["created_at"]);

		if ($isInserted === "True") // If user prompts already exists, send True to the client
			$tmp_array["is_sync"] = $isInserted;
		else
		{
			$saveStatus = $dao->saveCurrentUserPrompt($value["user_id"], $value["prompt_id"], $value["created_at"]);
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