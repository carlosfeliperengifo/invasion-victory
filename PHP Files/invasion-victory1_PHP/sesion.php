<?php
ob_start();

if (isset($_POST["usid"])) {
    $usid   = $_POST["usid"];
    $date   = $_POST["date"];
    $time   = $_POST["time"];
    
    require "conn_db.php";
    $mysqli = $conn;
    
    $sql    = "SELECT * from tb_sesion where (us_id = $usid AND se_date = '$date' AND se_hour = '$time')";
    $result = $mysqli->query($sql);
    $nfilas = $result->num_rows;
    
    if ($nfilas > 0) {
        $col = $result->fetch_array(MYSQLI_NUM);
        echo "1:" . $col[0];
    } else {
        $sql    = "INSERT INTO tb_sesion (se_date, se_hour, us_id) VALUES ('$date', '$time', $usid)";
        $result = $mysqli->query($sql);
        if(!$result) {
            echo "2"; // Error registrando sesion
        } else {
            $sql    = "SELECT * from tb_sesion where us_id = $usid";
            $result = $mysqli->query($sql);
            while ($col = $result->fetch_row()) {
                $seid   = $col[0];
            }
            echo "1:" . $seid; // Sesión registrada
        }
    }
    $mysqli->close();
}

ob_end_flush();
?>
