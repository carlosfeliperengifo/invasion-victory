<?php
ob_start();

if (isset($_POST['maid'])) {
    $compGame   = $_POST['completedGame'];
    $score      = $_POST['score'];
    $destSpa    = $_POST['destroyedSpaceships'];
    $dest7Total = $_POST['destroyed7Total'];
    $lifeBar    = $_POST['lifeBar'];
    $timePlayed = $_POST['timePlayed'];
    $seid       = $_POST['seid'];
    $maid       = $_POST['maid'];
    
    require "conn_db.php";
    $mysqli = $conn;
    
    $sql = "INSERT INTO tb_performance (pe_completedGame, pe_score, pe_destroyedSpaceships, pe_destroyed7Total, pe_lifeBar, pe_timePlayed, se_id, ma_id) VALUES ($compGame, $score, $destSpa, $dest7Total, $lifeBar, $timePlayed, $seid, $maid)";
    $result = $mysqli->query($sql);
    
    if (!$result) {
        echo "1"; // Error
    } else {
        echo "2"; // Performance insertado
    }
    $mysqli->close();
}

ob_end_flush();
?>
