<?php
ob_start();

if (isset($_POST["usid"]) && isset($_POST["option"])) {
    $usid   = $_POST["usid"];
    $option = $_POST["option"];
    
    require "conn_db.php";
    $mysqli = $conn;
    
    switch ($option) {
        case "return":
            $sql = "SELECT ra_id, us_points, us_weapon FROM tb_user WHERE us_id = $usid";
            $result = $mysqli->query($sql);
            $col = $result->fetch_array(MYSQLI_NUM);
            echo "1&&" . $col[0] . "&&" . $col[1] . "&&" . $col[2]; // ranking updated
        break;
        case "update":
            $inner = "INNER JOIN tb_sesion ON tb_sesion.se_id = tb_performance.se_id INNER JOIN tb_user ON tb_user.us_id = tb_sesion.us_id ";
            $sql = "SELECT pe_score FROM tb_performance " . $inner . " WHERE tb_user.us_id = $usid ORDER BY pe_id DESC LIMIT 10";
            $result = $mysqli->query($sql);
            $nFilas = $result->num_rows;
            
            if ($nFilas > 0) {
                $totalScore = 0;
                while ($col = $result->fetch_row()) {
                    $totalScore += $col[0];
                }
                
                $points = $totalScore/$nFilas;
                $points = round($points);
                
                $sql = "SELECT ra_id FROM tb_ranking WHERE ra_value <= $points ORDER BY ra_id DESC LIMIT 1";
                $result = $mysqli->query($sql);
                
                if ($result->num_rows > 0) {
                    $col  = $result->fetch_array(MYSQLI_NUM);
                    $raid = $col[0]; // new ranking
                    
                    $sql = "UPDATE tb_user SET ra_id = $raid, us_points = $points WHERE us_id = $usid";
                    
                    if ($mysqli->query($sql) === TRUE) {
                        $col = $result->fetch_array(MYSQLI_NUM);
                        echo "1&&" . $raid . "&&" . $points; // ranking updated
                    } else {
                        echo "2"; // Error update
                    }
                } else {
                    echo "2" . $mysqli->error; // Error ranking
                }
            } else {
                echo "1&&1&&0"; // null ranking
            }
        break;
        default:
            echo "3"; // invalid option
    }
    
    $mysqli->close();
}

ob_end_flush();
?>
