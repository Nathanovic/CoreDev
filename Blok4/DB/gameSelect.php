<html>
<head>
	<title>Game Select</title>
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
		<form action="scoreInsert.php" method="post">	
			<select name="gameSelect">
				<?php
				foreach($gameData as $game){
				?>
					<option value="<?php echo $game['id'] ?>"><?php echo $game['name'] ?></option>	
				<?php
				}
				?>
			</select>
			<input type="submit" value="Submit">
		</form>
	</div>
</body>
</html>