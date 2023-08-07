<?php
ob_start();

require "conn_db.php";
$mysqli = $conn;

$id = 164;

$sql = "SELECT us_id, us_nick FROM tb_user WHERE us_id = " . $id;
$result = $mysqli->query($sql);
$col = $result->fetch_array(MYSQLI_NUM);

echo $col[0];
echo ' - ' . $col[1];;
echo "<br>";

$result = $mysqli->query($sql);
$nrows = $result->num_rows;
if ($nrows > 0) {
    $inner = "INNER JOIN tb_sesion ON tb_sesion.se_id = tb_performance.se_id INNER JOIN tb_user ON tb_user.us_id = tb_sesion.us_id ";
    $sub_date = "concat_ws(' ', se_date, se_hour) BETWEEN '2023-05-07 00:00:01' AND '2023-05-23 04:00:00'";
    //$sub_score = "(select max(pe_score) FROM tb_performance " . $inner . "WHERE tb_user.us_id = " . $id . " AND " . $sub_date . ")";
    $sql = "SELECT pe_id, pe_completedGame, pe_score, pe_destroyedSpaceships, pe_lifeBar, pe_timePlayed, ma_id, tb_sesion.se_id, se_date, se_hour FROM tb_performance " . $inner . "WHERE tb_user.us_id = " . $id . " AND " . $sub_date;
    
    $result = $mysqli->query($sql);
    
    while ($col = $result->fetch_row()) {
        echo $col[0];
        echo ' & ' . $col[1];
        echo ' & ' . $col[2];
        echo ' & ' . $col[3];
        echo ' & ' . $col[4];
        echo ' & ' . $col[5];
        echo ' & ' . $col[6];
        echo ' & ' . $col[7];
        echo ' & ' . $col[8];
        echo ' & ' . $col[9];
        echo "<br>";
    }
    /*
    echo $col[0];
    echo ' && ' . $col[1];
    echo ' && ' . $col[2];
    echo ' && ' . $col[3];
    echo ' && ' . $col[4];
    echo ' && ' . $col[5];
    echo ' && ' . $col[6];
    echo ' && ' . $col[7];
    echo "<br>";*/
}

$mysqli->close();

ob_end_flush();
?>
