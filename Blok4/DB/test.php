<html>
<head>
	<title>Queries testing</title>
</head>
<body>
<?php
	require 'connect.php';
	
	//HackPasswordOfUser($mysqli, 1, "vogel");
	function HackPasswordOfUser($mysqli, $userID, $newPassword){
		$query = 
		"UPDATE  users
		SET  password = '".$newPassword."' 
		WHERE  id = '".$userID."'";
		
		$result = GetResult($mysqli, $query);
		if ($result != null){
			echo "succesfully updated user password to $newPassword</br>";
		}
	}
	
	//AddUser($mysqli, "mail@mail.com", "PassWooord");
	function AddUser($mysqli, $mail, $password){
		$query =
		"INSERT INTO users (
			id,
			mail,
			password,
			subscribeDate
		)
		VALUES (
		NULL, '$mail', '".$password."', NOW()
		)";
		
		$result = GetResult($mysqli, $query);
		if ($result != null){
			echo "succesfully added user: $mail</br>";
		}
	}
	
	ShowUsers($mysqli);
	function ShowUsers($mysqli){
		$query = "SELECT * FROM users";
		
		$result = GetResult($mysqli, $query);
		if ($result != null){
			$row = $result->fetch_assoc();
			do{
				echo json_encode($row)."</br>";
				//echo $row["id"].": ".$row["mail"]."</br>";		
			}
			while ($row = $result->fetch_assoc());
		}
	}
	
	/*
	$row = $result->fetch_assoc();
	do{
		echo $row["mail"]."_id: ".$row["id"];		
	}
	while ($row = $result->fetch_assoc());
	*/
?>

</body>
</html>