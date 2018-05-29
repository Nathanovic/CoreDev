<?php
	require 'connect.php';
	
	//this script is used to return a user id if there is one
	
	$userID = GetUserID();
	$userName = "";
	
	function GetUserID(){
		if(isset($_SESSION["userID"]) && $_SESSION["userID"] != ""){
			return $_SESSION["userID"];
		}
		
		return null;
	}
	
	if($userID == null){
		header('Location: /~nathan.flier/login');
	}
	else{
		$userName = $_SESSION["userName"];
		echo "User $userName($userID) is logged in!</br>";
	}
?>