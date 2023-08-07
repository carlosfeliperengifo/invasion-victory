<?php
ob_start();

if (isset($_POST["nick"])) {
    require "conn_db.php";
    $mysqli = $conn;
    
    $sql = "SELECT * FROM tb_questions";
    $result = $mysqli->query($sql);
    $nfilas = $result->num_rows;
    
    if($nfilas > 0) {
        while ($col = $result->fetch_row()) {
           echo $col[0] . '&&' . $col[1] . '%%';
        }
    }
    
    $mysqli->close();
}
ob_end_flush();
?>