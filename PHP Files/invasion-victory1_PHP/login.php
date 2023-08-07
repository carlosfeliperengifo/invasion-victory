<?php
ob_start();

if (isset($_POST["nick"])) {
    $nick   = $_POST["nick"];
    $pass   = $_POST["password"];
    
    require "conn_db.php";
    $mysqli = $conn;
    
    $sql    = "SELECT * FROM tb_user WHERE BINARY us_nick = '$nick'";
    $result = $mysqli->query($sql);
    $nfilas = $result->num_rows;
    
    if ($nfilas > 0) {
        $col    = $result->fetch_array(MYSQLI_NUM);
        
        if ($pass == $col[2]) {
            $usid   = $col[0];
            $arqu    = $col[3];
            $date   = date("Y/m/d");
            $time   = date("H:i:s");
            echo "1♦►" . $usid . "♦►" . $arqu . "♦►" . $date . "♦►" . $time;   // Login Ok
        } else {
            echo "2";   // Incorrect pass
        }
    } else {
        echo "3";   // Nick no found
    }
    $mysqli->close();
}

ob_end_flush();
?>
