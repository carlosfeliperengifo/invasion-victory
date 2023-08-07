<?php
ob_start();

if (isset($_POST["nick"])) {
    $nick   = $_POST["nick"];
    $pass   = $_POST["password"];
    $quid   = $_POST["quid"];
    $answer = $_POST["answer"];
    
    require "conn_db.php";
    $mysqli = $conn;
    
    $sql    = "SELECT * FROM tb_user WHERE BINARY us_nick = '$nick'";
    $result = $mysqli->query($sql);
    $nfilas = $result->num_rows;
    
    if ($nfilas > 0) {
        $col = $result->fetch_array(MYSQLI_NUM);
        if ($quid == $col[4]) {
            if ($answer == $col[5]) {
                $sql = "UPDATE tb_user SET us_password = '$pass' WHERE us_nick = '$nick'";
                $result = $mysqli->query($sql);
                if (!$result) {
                    echo "2"; // Error update
                } else {
                    echo "1"; // password changed
                }
            } else {
                echo "3"; // incorrect answer
            }
        } else {
            echo "4"; // incorrect question
        }
    } else {
        echo "5"; // nick no found
    }
    
    $mysqli->close();
}

ob_end_flush();
?>
