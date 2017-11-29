<?php 

require("database.php");
require("mysqldao.php");

$jsondata = "php://input";

// Get content of posted JSON String
$phpjsonstring = file_get_contents( $jsondata);

// Decoding content of posted JSON String
$data = json_decode($phpjsonstring, true);

$first_name = $data["first_name"];
$last_name = $data["last_name"];
$email = $data["email"];
$password = $data["password"];
$provider = $data["provider"];

//$email = htmlentities($_POST["email"]);
//$password = htmlentities($_POST["password"]);

$returnValue = array();

// Validation, if fields are not empty (Validation also provides on client side)
if(empty($first_name) || empty($last_name) || empty($email) || empty($provider))
{
	$returnValue["status"] = "error";
	$returnValue["message"] = "Missing required field";
	echo json_encode($returnValue);
	return;
}

// Creating instance of MySQLDao class
$dao = new MySQLDao();

$dao->openConnection();

// Getting user's provider by email
$userDetails = $dao->getUserRegisterData($email);

if(!empty($userDetails))
{
	// If user already has an account with Facebook authorisation and tries to login again, send its id
	if ($userDetails["provider"] == "Facebook" && $provider == "Facebook")
	{
		$returnValue["status"] = "Success";
		$returnValue["user_id"] = $userDetails["id"];
	}
	else
	{
		$returnValue["status"] = "Error";
		if ($userDetails["provider"] == "Email")
			$returnValue["message"] = "Email already exists";
		elseif ($userDetails["provider"] == "Facebook")
			$returnValue["message"] = "You are already registered via " . $userDetails["provider"];
	}
	echo json_encode($returnValue);
	return;
}

// Generating user's GUID
function getGUID(){
    if (function_exists('com_create_guid')){
        return com_create_guid();
    }
    else {
        mt_srand((double)microtime()*10000); //optional for php 4.2.0 and up.
        $charid = strtoupper(md5(uniqid(rand(), true)));
        $hyphen = chr(45);// "-"
        $uuid = //chr(123)// "{"
            substr($charid, 0, 8).$hyphen
            .substr($charid, 8, 4).$hyphen
            .substr($charid,12, 4).$hyphen
            .substr($charid,16, 4).$hyphen
            .substr($charid,20,12);
            //.chr(125);// "}"
        return $uuid;
    }
}

$guid = getGUID();

// Hashing user password if it's not empty (can be empty in case Facebook registration)
if (!empty($password))
	$secure_password = password_hash($password, PASSWORD_BCRYPT); 

$result = $dao->registerUser($guid, $first_name, $last_name, $email, $secure_password, $provider);

if($result)
{
	// Getting user's id after inserting (use in Facebook authorisation)
	$userId = $dao->getUserRegisterData($email);

	$returnValue["status"] = "Success";
	$returnValue["message"] = "Registration complete! Please proceed to Sign In";
	$returnValue["user_id"] = $userId["id"];
}
else
{
	$returnValue["status"] = "Error";
	$returnValue["message"] = "Could not successfully perform this request. Please try again later";
}

echo json_encode($returnValue);

if (json_last_error() !== JSON_ERROR_NONE)
	echo json_last_error_msg();

$dao->closeConnection();

?>