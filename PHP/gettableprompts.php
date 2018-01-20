<?php 

//
//  GetTablePrompts.php
//  Happy31.iOSApp
//
//  Copyright © 2017 Denis Klyucherov. All rights reserved.
//
//
// Sync table "prompts"

require("database.php");
require("mysqldao.php");

$returnValue = array();

// Creating instance of MySQLDao class
$dao = new MySQLDao();

$dao->openConnection();

// Getting table Prompts
$tablePrompts = $dao->getTablePrompts();

if(!empty($tablePrompts))
{
	$tmp_array = array();
	while ($res = mysqli_fetch_assoc($tablePrompts))
	{
		//$returnValue[] = $res;
		$tmp_array["prompt_id"] = $res["id"];
		$tmp_array["prompt_category"] = $res["category"];
		$tmp_array["prompt_task"] = mb_convert_encoding($res["task"], "UTF-8");
		$returnValue[] = $tmp_array;
	}
}
else
{
	$returnValue["status"] = "Error";
	$returnValue["message"] = "There is no prompts in table";
}

echo json_encode($returnValue);

if (json_last_error() !== JSON_ERROR_NONE)
	echo json_last_error_msg();
	
$dao->closeConnection();

?>