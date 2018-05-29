<?php
	//session_id('dl7rmblm6q0ggf2vraq9b0vum0');
	if(isset($_POST['PHPSESSID'])){
		$sid = htmlspecialchars($_POST['PHPSESSID']);
		session_id($sid);
	}
	else{
		header('Location: /~nathan.flier/login');
	}
	session_start();
	require 'connect.php';
		
	/*
	if(strtoupper($_SERVER["REQUEST_METHOD"]) == "POST"){
		$mail = $_POST["mail"];
		$password = $_POST["password"];
		$mailValidation = ValidateEmail($mail);
	*/		
	
	//login
	//validate login 
	//if true -> return user information + SESSION_ID()
	//on send new data -> try 
	
	ShowHighScores($mysqli, 1, 4);
	function ShowHighScores($mysqli, $gameID, $resultLimit){
		$query = 	"SELECT u.id, u.name, s.score 
					FROM scores s LEFT JOIN users u
					ON s.userID = u.id
					WHERE s.gameID = $gameID
					ORDER BY s.score DESC
					LIMIT $resultLimit";
		
		$result = GetResult($mysqli, $query);
		if($result != null){
			$row = $result->fetch_assoc();
			do{
				//echo $row["name"].": ".$row["score"]."</br>";
				echo json_encode($row);
				//echo $row["id"].": ".$row["mail"]."</br>";		
			}
			while ($row = $result->fetch_assoc());			
		}
	}
?>