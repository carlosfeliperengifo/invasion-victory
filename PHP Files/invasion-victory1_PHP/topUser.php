<?php
ob_start();

require "conn_db.php";
$mysqli = $conn;

$sql = "SELECT us_id FROM tb_user ORDER BY us_id";
$result = $mysqli->query($sql);
$n = 0;
while ($all = $result->fetch_row()) {
    $usids[$n] = $all[0];
    $n++;
}
for ($x = 0; $x < $n; $x++) {
    $inner = "INNER JOIN tb_sesion ON tb_sesion.se_id = tb_performance.se_id INNER JOIN tb_user ON tb_user.us_id = tb_sesion.us_id ";
    //$sub_date = "concat_ws(' ', se_date, se_hour) BETWEEN '2023-05-08 00:00:00' AND '2023-05-23 04:00:00'";
    $sub_score = "(select max(pe_score) FROM tb_performance " . $inner . "WHERE tb_user.us_id = " . $usids[$x] . ")";
    $sql = "SELECT us_nick, pe_score, pe_id FROM tb_performance " . $inner . "WHERE pe_score = " . $sub_score . " AND tb_user.us_id = " . $usids[$x];
    $result = $mysqli->query($sql);
    $nrows = $result->num_rows;
    if ($nrows > 0) {
        $col = $result->fetch_array(MYSQLI_NUM);
        echo $col[0];
        echo '&&' . $col[1];
        echo '%%';
    }
}

$mysqli->close();

ob_end_flush();
?>
