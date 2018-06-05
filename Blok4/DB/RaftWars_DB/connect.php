<?php
   $db_user = 'nathanflier';
   $db_pass = 'phathoox6E';
   $db_host = 'localhost';
   $db_name = 'nathanflier';

	/* Open a connection */
	$mysqli = new mysqli("$db_host","$db_user","$db_pass","$db_name");
	
	/* check connection */
	if ($mysqli->connect_errno) {
		echo "ERROR: Failed to connect to MySQL: (" . $mysqli->connect_errno() . ") " . $mysqli->connect_error();
		exit();
	}
	
	function GetResult($mysqli, $query){
		$result = $mysqli->query($query);
		if (!$result){
			ShowError($mysqli->errno,$mysqli->error);
			return null;
		}
		
		return $result;
	} 

	function ShowError($error,$errornr) {
		die("ERROR: (" . $errornr . ") " . $error);
	}
?>