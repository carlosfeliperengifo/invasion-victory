<?php
ob_start();

if (isset($_POST["serial"])) {
    $serial = $_POST["serial"];
    
    require "conn_db.php";
    $mysqli = $conn;
    
    $sql    = "SELECT * from tb_match where ma_serial = '$serial'";
    $result = $mysqli->query($sql);
    $nfilas = $result->num_rows;
    
    if ($nfilas > 0) {
        $col = $result->fetch_array(MYSQLI_NUM);
        echo "1:" . $col[0];
    } else {
        $shipsHorde = $_POST['spaceshipsHorde'];
        $timeHorde  = $_POST['timeHorde'];
        $speedShips = $_POST['speedSpaceships'];
        
        $sql = "INSERT INTO tb_match (ma_serial, ma_spaceshipsHorde, ma_timeHorde, ma_speedSpaceships) VALUES ('$serial', $shipsHorde, $timeHorde, $speedShips)";
        $result = $mysqli->query($sql);
        
        if (!$result) {
            echo "2";   // Error
        } else {
            $sql    = "SELECT * from tb_match where ma_serial = '$serial'";
            $result = $mysqli->query($sql);
            $nfilas = $result->num_rows;
            if ($nfilas > 0) {
                $col = $result->fetch_array(MYSQLI_NUM);
                echo "1:" . $col[0];
            } else {
                echo "3";
            }
        }
    }
    $mysqli->close();
}

ob_end_flush();
?>
