<?php
	session_start();
	require 'connect.php';	

	if(strtoupper($_SERVER["REQUEST_METHOD"]) == "POST"){
		$mail = $_POST["mail"];
		$password = $_POST["password"];
		$mailValidation = ValidateEmail($mail);
		$passwordValidation = ValidatePassword($password);
		if($mailValidation === true && $passwordValidation === true){
			$userRow = GetUserData($mysqli, $mail);
			if($userRow == null){
				InvalidInputError("Unknown email");
			}
			else{
				if($userRow["password"] != $password){
					InvalidInputError("Wrong password");
				}
				else{
					$userRow += [ "PHPSESSID" => SESSION_ID ];
					echo json_encode($userRow);
				}
			}
		}
		else{
			if($mailValidation !== true){
				InvalidInputError($mailValidation);
			}
			if($passwordValidation !== true){
				InvalidInputError($passwordValidation);
			}
		}
	}
	else{
		InvalidInputError("no POST input given!");
	}
	
	function ValidateEmail($mail){
		if($mail == ""){
			return "Email is required";
		}
		
		if(!filter_var($mail, FILTER_VALIDATE_EMAIL) === true){
			return "Invalid mail format";
		}
		
		return true;
	}
	function ValidatePassword($password){
		if(!preg_match("/^[a-zA-Z0-9]*$/",$password)) {
			return "Only letters and numbers allowed";
		}
		
		return true;
	}
	
	function GetUserData($mysqli, $mail){
		$query = "SELECT * FROM users
		WHERE mail = '$mail' LIMIT 1";
		
		$result = GetResult($mysqli, $query);
		if ($result != null){
			$row = $result->fetch_assoc();
			return $row;
		}
		else{
			return null;
		}
	}
	
	function InvalidInputError($error){
		echo "Invalid Input: ".$error."\n";
	}
?>