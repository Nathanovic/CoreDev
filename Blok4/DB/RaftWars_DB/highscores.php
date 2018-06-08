<?php
	$gameID = 0;
	$resultLimit = 0;
	if(strtoupper($_SERVER["REQUEST_METHOD"]) == "POST" && isset($_POST['PHPSESSID'])){
		$sid = htmlspecialchars($_POST['PHPSESSID']);
		session_id($sid);
		$resultLimit = intval($_POST["limit"]);
	}
	else{
		echo "ERROR: invalid session";
	}
	session_start();
	require 'connect.php';
	$gameID = $_SESSION["gameID"];
	date_default_timezone_set ('Europe/Amsterdam');
	
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
		$extraCondition = GetExtraCondition();
		$query = 	"SELECT u.id, u.name, s.score 
					FROM scores s LEFT JOIN users u
					ON s.userID = u.id
					WHERE s.gameID = $gameID
					$extraCondition
					ORDER BY s.score DESC
					LIMIT $resultLimit";
		
		$result = GetResult($mysqli, $query);
		if($result != null){
			$resultArr = array();
			$row = $result->fetch_assoc();
			do{
				array_push($resultArr, $row);		
			}
			while ($row = $result->fetch_assoc());		

			return json_encode($resultArr);
		}
		else{
			return null;
		}
	}
	
	function GetExtraCondition(){
		if(isset($_POST["condition"]) && $_POST["condition"] != ""){
			if($_POST["condition"] == "today"){
				$format = mktime(date("H"), date("i"), date("s"), date("m"), date("d") - 1, date("Y"));
				$yesterday = strftime("%Y-%m-%d", $format);
				return " AND date > '$yesterday' ";
			}
			else{
				echo "ERROR: unkown condition: ".$_POST["condition"];
			}
		}
		
		return "";		
	}
	
	echo $highscoreJSON;
?>