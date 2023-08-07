<?php
ob_start();

if (isset($_POST["nick"])) {
    $nick   = $_POST["nick"];
    $survey = "https://forms.gle/BLpGcvGB4gWm4kAF9";
    $minGames = 2;
    
    require "conn_db.php";
    $mysqli = $conn;
    
    $inner = "INNER JOIN tb_sesion ON tb_sesion.se_id = tb_performance.se_id INNER JOIN tb_user ON tb_user.us_id = tb_sesion.us_id";
    $sql = "SELECT COUNT(pe_id) FROM tb_performance $inner WHERE us_nick = '$nick'";
    $result = $mysqli->query($sql);
    $col = $result->fetch_array(MYSQLI_NUM);
    
    if ($col[0] >= $minGames) {
        echo "1&&" . $survey;
    } else {
        echo"2";
    }
    
    $mysqli->close();
}

ob_end_flush();
?>