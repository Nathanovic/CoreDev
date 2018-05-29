<html>
<head>
	<title>Score Insert</title>
</head>
<body>
	<?php
		session_start();
		require 'userValidation.php';
		
		if(isset($_POST['gameSelect'])){
			$game = $_POST['gameSelect'];
			echo "selected game: $game";
		}
		else{
			echo "ERROR: no game selected";
		}
	?>
</body>
</html>