<?php

//
//  UserLogin.php
//  Happy31.iOSApp
//
//  Copyright © 2017 Denis Klyucherov. All rights reserved.
//
//
// Login user
// INPUT: email, password, provider
// OUTPUT: user_id, first_name, last_name, email, provider, avatar, created_at, status, message (if "Error")

require("database.php");
require("mysqldao.php");

$jsondata = "php://input";

// Get content of posted JSON String
$phpjsonstring = file_get_contents( $jsondata);

// Decoding content of posted JSON String
$data = json_decode($phpjsonstring, true);

$email = $data["email"];
$password = $data["password"];

$returnValue = array();

// Validation, if fields are not empty (Validation also provides on client side)
if(empty($email) || empty($password))
{
	$returnValue["status"] = "error";
	$returnValue["message"] = "Missing required field";
	echo json_encode($returnValue);
	return;
}

// Creating instance of MySQLDao class
$dao = new MySQLDao();

$dao->openConnection();

// Getting user's id and password by email
$userDetails = $dao->getUserLoginData($email);

if(!empty($userDetails))
{
	if ($userDetails["provider"] == "Facebook")
	{
		$returnValue["status"] = "Error";
		$returnValue["message"] = "You are already registered. Please, use Facebook to Log In";
	}
	else
	{
		// Dehashing user password
		$check_password = password_verify($password, $userDetails["password"]); 

		if ($check_password)
		{
			$returnValue["status"] = "Success";
			$returnValue["user_id"] = $userDetails["id"];
			$returnValue["first_name"] = $userDetails["first_name"];
			$returnValue["last_name"] = $userDetails["last_name"];
			$returnValue["email"] = $email;
			$returnValue["provider"] = $userDetails["provider"];
			$returnValue["avatar"] = $userDetails["avatar"];
			$returnValue["created_at"] = $userDetails["created_at"];
		}
		else
		{
			$returnValue["status"] = "Error";
			$returnValue["message"] = "Invalid password";
		}
	}
}
else
{
	$returnValue["status"] = "Error";
	$returnValue["message"] = "The email is not registered in the system";
}

echo json_encode($returnValue);

if (json_last_error() !== JSON_ERROR_NONE)
	echo json_last_error_msg();

$dao->closeConnection();

?>