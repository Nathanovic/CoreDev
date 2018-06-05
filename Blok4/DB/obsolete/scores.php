<html>
<head>
	<title>Scores</title>
</head>
<body>
<?php
	require 'connect.php';
	
	//ShowScore($mysqli, 1);
	function ShowScore($mysqli, $gameID){
		$query = "SELECT * FROM scores
		WHERE gameID = $gameID";
		
		$result = GetResult($mysqli, $query);
		if ($result != null){
			$row = $result->fetch_assoc();
			do{
				echo json_encode($row);
				//echo $row["id"].": ".$row["mail"]."</br>";		
			}
			while ($row = $result->fetch_assoc());
		}
	}
	
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
				echo $row["name"].": ".$row["score"]."</br>";
				//echo json_encode($row);
				//echo $row["id"].": ".$row["mail"]."</br>";		
			}
			while ($row = $result->fetch_assoc());			
		}
	}
?>
</body>
</html>