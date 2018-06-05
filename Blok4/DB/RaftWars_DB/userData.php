<?php
	$resultLimit = 0;
	if(strtoupper($_SERVER["REQUEST_METHOD"]) == "POST" && isset($_POST['PHPSESSID'])){
		$sid = htmlspecialchars($_POST['PHPSESSID']);
		session_id($sid);
	}
	else{
		echo "ERROR: invalid session";
	}
	
	session_start();
	require_once 'connect.php';
	$userID = $_SESSION["userID"];
	
	if(isset($_POST["newPassword"])){
		$newPassword = $_POST["newPassword"];
		$passwordValidation = ValidatePassword($newPassword);
		if($passwordValidation === TRUE){
			UpdateUserData($mysqli, $userID, "password", $newPassword);
		}
		else{
			echo $passwordValidation;
		}
	}	
	else if(isset($_POST["newMail"])){
		$newMail = $_POST["newMail"];
		$mailValidation = ValidateEmail($newMail);
		if($mailValidation === TRUE){
			UpdateUserData($mysqli, $userID, "mail", $newMail);
		}
		else{
			echo $mailValidation;
		}
	}
	else{
		echo "WARNING: POST has not been set correctly";
	}
	
	function UpdateUserData($mysqli, $userID, $column, $value){
		$query = "UPDATE users
				SET $column = '$value'
				WHERE id = $userID";
				
		if($mysqli->query($query) !== TRUE){
			echo "ERROR: ".$query.": ".$mysqli->error;
		}
		else{
			echo "You have succesfully updated your $column!";
		}
	}
	
	function ValidateEmail($mail){
		if($mail == ""){
			return "The field is empty";
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
?>