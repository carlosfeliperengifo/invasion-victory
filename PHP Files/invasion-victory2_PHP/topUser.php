<?php
ob_start();

if (isset($_POST["raid"])) {
    $raid = $_POST["raid"];
    
    require "conn_db.php";
    $mysqli = $conn;
    
    $sql = "SELECT us_nick, us_points FROM tb_user WHERE us_points > 0 && ra_id = $raid ORDER BY us_points DESC";
    $result = $mysqli->query($sql);
    $nrows = $result->num_rows;
    
    if ($nrows > 0) {
        while ($col = $result->fetch_row()) {
            echo $col[0] . '&&' . $col[1] . '%%';
        }
    }
    
    $mysqli->close();
}

ob_end_flush();
?>
