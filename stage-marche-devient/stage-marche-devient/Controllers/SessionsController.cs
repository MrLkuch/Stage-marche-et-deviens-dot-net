﻿using Microsoft.AspNetCore.Mvc;
using stage_marche_devient.Data;
using stage_marche_devient.Models;
using stage_marche_devient.Repositories;

namespace stage_marche_devient.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SessionController : ControllerBase
    {
        private readonly ApiDbContext _context;
        private readonly SessionRepository _repository;
        private readonly ILogger<SessionController> _logger;

        public SessionController(ApiDbContext context, ILogger<SessionController> logger, ILogger<SessionRepository> sessionLogger)
        {
            _context = context;
            _repository = new SessionRepository(_context, sessionLogger);
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Session>>> GetAllSession()
        {
            var session = await _repository.GetAll();                                                         //recupere depuis le repo ttes les données
            return Ok(session);                                                                               // retourne vers le front les données récupérees
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Session>> GetSession(int id)
        {
            var session = await _repository.GetById(id);                                                      //recupere les données depuis le repo
            if (session == null)                                                                              //si les données sont null on found vers le endpoint
            {
                return NotFound();                                                                              //retourne un not found vers le endpoint (code 404)
            }
            return Ok(session);                                                                               //retourne vers le front les données récupéree(code 200)
        }

        [HttpPost]

        public async Task<ActionResult<Session>> CreateSession([FromBody] Session model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    bool result = await _repository.Add(model);
                    if (result)
                    {
                        return Ok(model);
                    }
                    else
                    {
                        return BadRequest("Erreur lors de l'ajout de la session.");
                    }
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }

            }

                return BadRequest(ModelState);

                /*var result = await _repository.Add(session);                                                      //envoie vers le repo l'objet et stock le boolean de retour
                if (result)                                                                                         //si le boolean de retour est true
                {
                    return CreatedAtAction(nameof(GetSession), new { id = session.IdSession }, session);    // renvoi vers le endpoint l'objet qui vient d'être crée
                }
                return BadRequest();   */                                                                           // envoi un badresquest (code 400)
        }

        [HttpPut("{id}")]

        public async Task<ActionResult> UpdateSession(int id, Session session)
        {
            if (id != session.IdSession)                                                                    //vérifie si la donnée que l'on veut update est bien celle avec cet id
            {
                return BadRequest();                                                                            //rebvoie un bad request (code 400)
            }

            var result = await _repository.Update(session, id);                                               // envoi vers le repository l'objet a update et son id
            if (result)
            {
                return Ok("Modificaton réussie");                                                               // renvoi un ok(code 200)
            }

            return NotFound();                                                                                  // renvoie un not found (code 404)
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSession(int id,Session session)
        {
            
            var result = await _repository.Delete(id);                                                          // envoi un requete de deletion vers le repository et stock le retour
            if (result)                                                                                         // si le retour est positive
            {
                return Ok("Supression reussie");                                                                // revoi un ok (code ~200) 
            }

            _logger.LogError("Erreur lors de la suppression du thème avec ID {Id}", id);

            return NotFound();                                                                                  // revoie un not found (code 404)
        }

    }
}