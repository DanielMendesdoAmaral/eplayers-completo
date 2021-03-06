using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using e_players_completo.Models;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace e_players_completo.Controllers
{
    public class NoticiaController : Controller
    {
        //Instanciamos o nosso model, assim, teremos acesso ao nosso banco de dados e todos métodos do CRUD.
        Noticia noticiaModel = new Noticia();

        /// <summary>
        ///     Este método lê nosso bd e guarda em uma ViewBag. 
        /// </summary>
        /// <returns>Retorna a nossa view.</returns>
        public IActionResult Index()
        {
            ViewBag.Noticias = noticiaModel.ReadAll();
            return View();
        }

        /// <summary>
        ///     Cadastra no bd a partir de um form no front.
        /// </summary>
        /// <param name="form">Form do front.</param>
        /// <returns>Retorna para a mesma página após concluir a ação.</returns>
        public IActionResult Cadastrar(IFormCollection form) {

            Noticia noticia = new Noticia();
            noticia.IdNoticia = Convert.ToInt32( form["IdNoticia"] );
            noticia.Titulo = form["Titulo"];
            noticia.Texto = form["Texto"];

            //Aqui começa upload de imagens
            noticia.Imagem   = form["Imagem"];
            var file    = form.Files[0]; //Esta variável guarda o nome do arquivo da imagem.
            var folder  = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/Noticias"); //Esta variável combina o caminho do diretório atual com o "wwwroot/img/Noticias", onde ficará a imagem.

            //Se o usuário enviou alguma imagem..
            if(file != null)
            {
                //Vai verificar se existe a pasta, se não existir, cria.
                if(!Directory.Exists(folder)){
                    Directory.CreateDirectory(folder);
                }

                //Esta variável vai criar e guardar o caminho completo. Note que o Path.Combine ele faz mais do que unir duas variáveis. Por exemplo, se um parâmetro for temp e outro for temp.txt, o path combine será temp.txt.
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/", folder, file.FileName);
                
                //Usando FileStream, passa-se o caminho onde ficará a imagem e cria o arquivo.
                using (var stream = new FileStream(path, FileMode.Create))  
                {  
                    //Copia o arquivo guardado em "file" para o "stream".
                    file.CopyTo(stream);  
                }

                //Guarda-se o caminho da imagem no banco de dados.
                noticia.Imagem   = file.FileName;
            }
            else //<- Se o usuário não mandar um arquivo, salva-se o seguindo no bd:
            {
                noticia.Imagem   = "padrao.png";
            }
            //Fim

            noticiaModel.Create(noticia); //Insere o objeto criado no bd.

            ViewBag.Noticias = noticiaModel.ReadAll(); //Lê tudo do csv e guarda em uma lista. A ViewBag recebe esta lista. A viewbag é criada aqui.
            return LocalRedirect("~/Noticia"); //Está pagina, Noticia, é criada na view, pela sintaxe Razor.
        }

        //Adicionamos a rota do item a ser excluido.
        [Route("Noticia/{id}")]
        /// <summary>
        ///     Exclui um item da view e do bd.
        /// </summary>
        /// <param name="id">Id do item a ser excluído, que é enviado pela rota.</param>
        /// <returns>Retorna para a mesma página.</returns>
        public IActionResult Excluir(int id) {
            noticiaModel.Delete(id);
            ViewBag.Noticias = noticiaModel.ReadAll();
            return LocalRedirect("~/Noticia");
        }
    }
}