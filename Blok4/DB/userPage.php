<html>
<head>
	<title>User page</title>
</head>
<body>
	<?php
		session_start();
		require 'userValidation.php';
		
		$gameData = GetGameData($mysqli);
		function GetGameData($mysqli){
			$query = "SELECT * FROM games";
			$result = GetResult($mysqli, $query);
			
			$rows = array();
			while($row = mysqli_fetch_array($result)){
				array_push($rows, $row);
			}
			
			return $rows;
		}
	?>
	<div id="container">
		<p>My data:</p>
		<form action="scoreInsert.php" method="post">	
			Password: <input type="text" name="f_Password" value="pssword"/></br>
			<select name="f_Sex">
				<option value="male">male</option>
				<option value="female">female</option>
			</select></br>
			<select name="f_favoriteGame">
				<?php
				foreach($gameData as $game){
				?>
					<option value="<?php echo $game['id'] ?>"><?php echo $game['name'] ?></option>	
				<?php
				}
				?>
			</select></br>
			<input type="submit" value="Submit">
		</form>
	</div>
</body>
</html>