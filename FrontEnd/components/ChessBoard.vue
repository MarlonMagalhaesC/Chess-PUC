<template>
  <div>
    <h2>Turno: {{ turno }}</h2>
    <p v-if="message" class="message">{{ message }}</p>

    <div class="chess-wrapper">
      <div class="top-left-empty"></div>
      <div class="column-label" v-for="letter in columnLetters" :key="letter">
        {{ letter }}
      </div>

      <template v-for="(row, rowIndex) in 8" :key="rowIndex">
        <div class="row-label">
          {{ 8 - rowIndex }}
        </div>

        <template v-for="col in 8" :key="col">
          <div
            :class="[
              'cell',
              getCellColor(rowIndex, col - 1),
              isSelected({ linha: rowIndex, coluna: col - 1 }) ? 'selected' : '',
              isPossibleMove({ linha: rowIndex, coluna: col - 1 }) ? 'possible' : ''
            ]"
            @click="selectCell({ linha: rowIndex, coluna: col - 1 })"
          >
            <div
              v-if="pieceAt(rowIndex, col - 1)"
              class="peca"
              :class="pieceAt(rowIndex, col - 1).cor === 'Branca' ? 'white-piece' : 'black-piece'"
            >
              {{ pieceAt(rowIndex, col - 1).peca }}
            </div>
          </div>
        </template>
      </template>
    </div>

    <div class="captured">
      <h3>Capturadas Brancas:</h3>
      <div class="captured-pieces">
        <span v-for="(piece, index) in capturedWhite" :key="index">
          {{ piece.peca }}
        </span>
      </div>

      <h3>Capturadas Pretas:</h3>
      <div class="captured-pieces">
        <span v-for="(piece, index) in capturedBlack" :key="index">
          {{ piece.peca }}
        </span>
      </div>
    </div>


    <button @click="resetGame" class="reset-button">Reiniciar Jogo</button>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'

const board = ref([])
const selected = ref(null)
const possibleMovesGrid = ref([])
const turno = ref('')
const message = ref('')
const capturedWhite = ref([])
const capturedBlack = ref([])

const columnLetters = ['a', 'b', 'c', 'd', 'e', 'f', 'g', 'h']

onMounted(() => {
  loadBoard()
  loadStatus()
  loadCaptured()
})

async function loadBoard() {
  const res = await fetch('https://chess-puc-2.onrender.com/Chess/board')
  board.value = await res.json()
}

async function loadStatus() {
  const res = await fetch('https://chess-puc-2.onrender.com/Chess/status')
  const data = await res.json()
  turno.value = data.turno
  if (data.terminada) {
    message.value = `♛ Xeque-mate! Vencedor: ${data.turno}`
  } else if (data.xeque) {
    message.value = `⚠️ Xeque em ${data.turno}`
  } else {
    message.value = ''
  }
}

async function loadCaptured() {
  const res = await fetch('https://chess-puc-2.onrender.com/Chess/captured')
  const data = await res.json()
  capturedWhite.value = data.brancas
  capturedBlack.value = data.pretas
}

async function loadPossibleMovesGrid(cell) {
  const res = await fetch('https://chess-puc-2.onrender.com/Chess/possible-moves-grid', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({
      FromCol: String.fromCharCode(97 + cell.coluna),
      FromRow: 8 - cell.linha
    })
  })

  if (res.ok) {
    possibleMovesGrid.value = await res.json()
  } else {
    possibleMovesGrid.value = []
  }
}

function selectCell(cell) {
  if (!selected.value) {
    const piece = pieceAt(cell.linha, cell.coluna)
    if (piece) {
      selected.value = cell
      loadPossibleMovesGrid(cell)
    }
  } else {
    makeMove(selected.value, cell)
    selected.value = null
    possibleMovesGrid.value = []
  }
}

async function resetGame() {
  const res = await fetch('https://chess-puc-2.onrender.com/Chess/reset', {
    method: 'POST'
  })

  if (res.ok) {
    await loadBoard()
    await loadStatus()
    await loadCaptured()
    message.value = '♻️ Jogo reiniciado com sucesso!'
  } else {
    message.value = '❌ Erro ao reiniciar o jogo.'
  }
}

async function makeMove(from, to) {
  const response = await fetch('https://chess-puc-2.onrender.com/Chess/move', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({
      FromCol: String.fromCharCode(97 + from.coluna),
      FromRow: 8 - from.linha,
      ToCol: String.fromCharCode(97 + to.coluna),
      ToRow: 8 - to.linha
    })
  })

  if (response.ok) {
    await loadBoard()
    await loadStatus()
    await loadCaptured()
    message.value = '✔️ Movimento realizado!'
  } else {
    const error = await response.json()
    message.value = error.error || '❌ Movimento inválido'
  }
}

function getCellColor(row, col) {
  return (row + col) % 2 === 0 ? 'white' : 'black'
}

function isSelected(cell) {
  return selected.value &&
    cell.linha === selected.value.linha &&
    cell.coluna === selected.value.coluna
}

function isPossibleMove(cell) {
  if (!possibleMovesGrid.value.length) return false
  return possibleMovesGrid.value[cell.linha][cell.coluna] === true
}

function pieceAt(row, col) {
  return board.value.find(c => c.linha === row && c.coluna === col)
}
</script>

<style scoped>
.chess-wrapper {
  display: grid;
  grid-template-columns: 30px repeat(8, 70px);
  grid-template-rows: 30px repeat(8, 70px);
  align-items: center;
  justify-content: center;
  margin: 30px auto;
}

.top-left-empty {
  width: 30px;
  height: 30px;
}

.column-label {
  width: 70px;
  height: 30px;
  display: flex;
  justify-content: center;
  align-items: center;
  font-weight: 700;
  color: #222;
}

.row-label {
  width: 30px;
  height: 70px;
  display: flex;
  justify-content: center;
  align-items: center;
  font-weight: 700;
  color: #222;
}

.cell {
  width: 70px;
  height: 70px;
  display: flex;
  justify-content: center;
  align-items: center;
  cursor: pointer;
  transition: transform 0.2s ease;
}

.cell:hover {
  transform: scale(1.05);
}

.white {
  background-color: #f0d9b5;
}

.black {
  background-color: #b58863;
}

.selected {
  background-color: #ffe600 !important;
}

.possible {
  outline: 3px solid limegreen;
  outline-offset: -3px;
}

.white-piece {
  color: white;
  font-weight: 900;
  font-size: 32px;
  text-shadow: 0 0 3px black;
}

.black-piece {
  color: black;
  font-weight: 900;
  font-size: 32px;
}

.message {
  margin-top: 12px;
  font-weight: bold;
  color: royalblue;
  text-align: center;
}

.captured {
  margin-top: 30px;
  text-align: center;
}

.captured h3 {
  margin-bottom: 8px;
  color: #333;
}

.captured-pieces {
  display: flex;
  gap: 8px;
  justify-content: center;
  flex-wrap: wrap;
  margin-bottom: 14px;
}

.captured-pieces span {
  font-size: 22px;
  font-weight: bold;
  border: 1px solid #333;
  padding: 6px 10px;
  background-color: #f9f9f9;
  border-radius: 6px;
  box-shadow: 1px 1px 4px rgba(0,0,0,0.25);
}

.peca {
  font-size: 50px;
}

.reset-button {
  margin-top: 30px;
  padding: 12px 16px;
  font-weight: bold;
  background-color: #333;
  color: white;
  border: none;
  border-radius: 6px;
  cursor: pointer;
  transition: background-color 0.3s ease;
}
.reset-button:hover {
  background-color: #555;
}
</style>
