<html>
<head>
	<title>Login</title>
</head>
<body>
<?php
	session_start();
	require 'connect.php';
	
	$mailValidation = $passwordValidation = "";
	$loginStyle = "";
	
	if(strtoupper($_SERVER["REQUEST_METHOD"]) == "POST"){
		$mail = $_POST["mail"];
		$password = $_POST["password"];
		$mailValidation = ValidateEmail($mail);
		$passwordValidation = ValidatePassword($password);
		if($mailValidation === true && $passwordValidation === true){
			$userRow = GetUserData($mysqli, $mail);
			if($userRow == null){
				$mailValidation = "Unknown email";
			}
			else{
				$requiredPassword = $userRow["password"];
				if($requiredPassword != $password){
					$passwordValidation = "Wrong password";
				}
				else{
					$userID = $userRow["id"];
					$_SESSION["userID"] = $userID;
					$_SESSION["userName"] = $userRow["name"];
					
					$loginStyle = "style='display: none;'";
					echo "user: $userID logged in! </br>";
					echo "session: ".SESSION_ID();
				}
			}
		}
		
		if($mailValidation === true){
			$mailValidation = "";
		}
		if($passwordValidation === true){
			$passwordValidation = "";
		}
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
?>
	<div id="login" <?php echo $loginStyle; ?>>
		<form action="#" method="POST">
			<table>
				<tr>
					<td><p>mail: </p></td>
					<td><input type="text" name="mail"></td>
					<td><span class="error">*<?php echo $mailValidation; ?></span></td>
				</tr>
				<tr>
					<td><p>password: </p></td>
					<td><input type="text" name="password"></td>
					<td><span class="error">*<?php echo $passwordValidation; ?></span></td>
				</tr>
			</table>
			<input type="submit" value="Log in"></input>
		</form>
		<span class="error">* = required</span>
	</div>
</body>
</html>