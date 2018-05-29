<?php
	//session_id('dl7rmblm6q0ggf2vraq9b0vum0');
	$gameID = 0;
	$resultLimit = 0;
	if(isset($_POST['PHPSESSID'])){
		$sid = htmlspecialchars($_POST['PHPSESSID']);
		session_id($sid);
		$resultLimit = intval($_POST["limit"]);
	}
	else{
		echo "ERROR: invalid session";
		//header('Location: /~nathan.flier/login');
	}
	session_start();
	$gameID = $_SESSION["gameID"];
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
	
	if(isset($_POST["achievedScore"])){
		$userID = $_SESSION["userID"];
		$score = $_POST["achievedScore"];
		InsertUserScore($mysqli, $userID, $gameID, $score);
	}
	
	function InsertUserScore($mysqli, $userID, $gameID, $score){
		$query = "INSERT INTO scores (gameID, userID, score)
				VALUES ($gameID, $userID, $score)";
		if($mysqli->query($query) !== TRUE){
			echo "ERROR: ".$query.": ".$mysqli->error;
		}
	}
		
	$highscoreJSON = GetHighscoreJSON($mysqli, $gameID, $resultLimit);
	function GetHighscoreJSON($mysqli, $gameID, $resultLimit){
		$query = 	"SELECT u.id, u.name, s.score 
					FROM scores s LEFT JOIN users u
					ON s.userID = u.id
					WHERE s.gameID = $gameID
					ORDER BY s.score DESC
					LIMIT $resultLimit";
		
		$result = GetResult($mysqli, $query);
		if($result != null){
			$resultArr = array();
			$row = $result->fetch_assoc();
			do{
				array_push($resultArr, $row);
				//echo $row["id"].": ".$row["mail"]."</br>";		
			}
			while ($row = $result->fetch_assoc());		

			return json_encode($resultArr);
		}
		else{
			return null;
		}
	}
	
	echo $highscoreJSON;
?>