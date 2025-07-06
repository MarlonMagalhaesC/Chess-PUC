<template>
  <div class="game">
    <h1>Jogo de Xadrez</h1>
    <ChessBoard />
  </div>
</template>

<script setup>
import ChessBoard from '~/components/ChessBoard.vue'
</script>

<style scoped>


.game {
    text-align: center;
    justify-content:center;
    gap: 50px;
}

h1 {
    font-family:Arial, Helvetica, sans-serif !important;
}
</style>
