<?php
ob_start();

if (isset($_POST["usid"])) {
    $usid = $_POST["usid"];
    
    require "conn_db.php";
    $mysqli = $conn;
    
    $inner = "INNER JOIN tb_sesion ON tb_sesion.se_id = tb_performance.se_id INNER JOIN tb_user ON tb_user.us_id = tb_sesion.us_id INNER JOIN tb_match ON tb_match.ma_id = tb_performance.ma_id ";
    $sql = "SELECT pe_completedGame, pe_destroyedSpaceships, ma_spaceshipsHorde FROM tb_performance $inner WHERE tb_user.us_id = $usid ORDER BY pe_id DESC LIMIT 1";
    $result = $mysqli->query($sql);
    $nrows = $result->num_rows;
    
    if ($nrows == 1) {
        $col = $result->fetch_array(MYSQLI_NUM);
        $compGame   = $col[0];
        $destShips  = $col[1];
        $shipsHorde = $col[2];
        
        if ($compGame == 1 && $destShips > round(0.8*5*$shipsHorde)) {
            $newShips = 1;
        } else if ($destShips < round(0.6*5*$shipsHorde)) {
            $newShips = -1;
        } else {
            $newShips = 0;
        }
    } else {
        $shipsHorde = 3;
        $newShips = 0;
    }
    
    echo "1&&" . $shipsHorde . "&&" . $newShips;
    
    $mysqli->close();
}

ob_end_flush();
?>
