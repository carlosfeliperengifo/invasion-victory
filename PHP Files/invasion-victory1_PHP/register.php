<?php
ob_start();

if (isset($_POST["nick"])) {
    $nick   = $_POST["nick"];
    $pass   = $_POST["password"];
    $quar   = $_POST["arqu"];
    $quid   = $_POST["quid"];
    $answer = $_POST["answer"];
    
    require "conn_db.php";
    $mysqli = $conn;
    
    $sql    = "SELECT * FROM tb_user WHERE BINARY us_nick = '$nick'";
    $result = $mysqli->query($sql);
    $nfilas = $result->num_rows;
    
    if ($nfilas > 0){
        echo "1";   // El nick ya está registrado
    } else {
        $sql = "INSERT INTO tb_user (us_nick, us_password, us_questionAR, qu_id, us_answer) VALUES ('$nick', '$pass', '$quar', $quid, '$answer')";
        $result = $mysqli->query($sql);
        if(!$result) {
            echo "2";   // Error registrando usuario
        } else {
            echo "3";   // Usuario registrado
        }
    }
    $mysqli->close();
}

ob_end_flush();
?>
