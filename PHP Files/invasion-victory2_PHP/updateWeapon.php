<?php
ob_start();

if (isset($_POST["usid"]) && isset($_POST["weapon"])) {
    $usid   = $_POST["usid"];
    $weapon = $_POST["weapon"];
    
    require "conn_db.php";
    $mysqli = $conn;
    
    $sql = "UPDATE tb_user SET us_weapon = '$weapon' WHERE us_id = $usid";
    
    if ($mysqli->query($sql) === TRUE) {
        echo "1"; // weapon updated
    } else {
        echo "2"; // Error update
    }
    
    $mysqli->close();
}

ob_end_flush();
?>