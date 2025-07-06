using Microsoft.AspNetCore.Mvc;
using ChessAPI.Models;

namespace ChessAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ChessController : ControllerBase
    {
        public static PartidaDeXadrez partida = new PartidaDeXadrez();

        [HttpGet("board")]
        public IActionResult GetBoard()
        {
            var board = new List<object>();

            for (int i = 0; i < partida.tab.linhas; i++)
            {
                for (int j = 0; j < partida.tab.colunas; j++)
                {
                    var peca = partida.tab.peca(i, j);
                    board.Add(new
                    {
                        linha = i,
                        coluna = j,
                        peca = peca?.ToString(),
                        cor = peca?.cor.ToString()
                    });
                }
            }

            return Ok(board);
        }

        [HttpGet("status")]
        public IActionResult GetStatus()
        {
            return Ok(new
            {
                turno = ChessController.partida.jogadorAtual.ToString(),
                xeque = ChessController.partida.xeque,
                terminada = ChessController.partida.terminada
            });
        }

        [HttpPost("possible-moves-grid")]
        public IActionResult GetPossibleMovesGrid([FromBody] MoveRequest move)
        {
            try
            {
                var posicao = new Posicao(8 - move.FromRow, move.FromCol - 'a');
                var peca = partida.tab.peca(posicao);

                if (peca == null)
                {
                    return BadRequest(new { error = "Não há peça na posição selecionada." });
                }

                var movimentos = peca.movimentosPossiveis();
                bool[,] matriz = movimentos;


                var grid = new List<List<bool>>();
                for (int i = 0; i < partida.tab.linhas; i++)
                {
                    var linha = new List<bool>();
                    for (int j = 0; j < partida.tab.colunas; j++)
                    {
                        linha.Add(matriz[i, j]);
                    }
                    grid.Add(linha);
                }

                return Ok(grid);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("captured")]
        public IActionResult GetCaptured()
        {
            var brancas = partida.pecasCapturadas(Cor.Branca)
                .Select(p => new { peca = p.ToString(), cor = "Branca" });

            var pretas = partida.pecasCapturadas(Cor.Preta)
                .Select(p => new { peca = p.ToString(), cor = "Preta" });

            return Ok(new { brancas, pretas });
        }




        [HttpPost("move")]
        public IActionResult Move([FromBody] MoveRequest move)
        {
            try
            {
                var origem = new Posicao(8 - move.FromRow, move.FromCol - 'a');
                var destino = new Posicao(8 - move.ToRow, move.ToCol - 'a');

                partida.validarPosicaoDeOrigem(origem);
                partida.validarPosicaoDeDestino(origem, destino);
                partida.realizaJogada(origem, destino);

                return Ok(new { message = "Movimento realizado com sucesso" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }



        [HttpPost("reset")]
        public IActionResult ResetGame()
        {
            partida = new PartidaDeXadrez();
            return Ok(new { message = "Jogo reiniciado com sucesso" });
        }

    }

    public class MoveRequest
    {
        public char FromCol { get; set; }
        public int FromRow { get; set; }
        public char ToCol { get; set; }
        public int ToRow { get; set; }
    }
}
