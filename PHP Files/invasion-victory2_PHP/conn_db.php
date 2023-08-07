<?php
  // Remplace 'user', 'password' and 'database'
  $conn = mysqli_connect(
    'localhost',
    'user',
    'password',
    'database'
  ) or die(mysqli_error($conn));
?>
