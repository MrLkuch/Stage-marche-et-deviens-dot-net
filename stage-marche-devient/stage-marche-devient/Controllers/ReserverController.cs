﻿using Microsoft.AspNetCore.Mvc;
using stage_marche_devient.Data;
using stage_marche_devient.Models;
using stage_marche_devient.Repositories;

namespace stage_marche_devient.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReserverController : ControllerBase
    {
        // Déclaration des champs privés pour le contexte de base de données et le repository
        private readonly ApiDbContext _contexteBdd;
        private readonly ReserverRepository _repository;

        // Constructeur du contrôleur
        public ReserverController(ApiDbContext context)
        {
            _contexteBdd = context;
            _repository = new ReserverRepository(context);
        }

        #region Récupération liste
        // Endpoint GET pour récupérer toutes les réservations
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ReserverModel>>> RecupererReservations()
        {
            var listeReservations = await _repository.GetAll();
            return Ok(listeReservations);
        }
        #endregion

        #region Récupération par id
        // Endpoint GET pour récupérer une réservation spécifique par ID utilisateur et ID session
        [HttpGet("{idUtilisateur},{idSession}")]
        public async Task<ActionResult<ReserverModel>> RecupererReservation(int idUtilisateur, int idSession)
        {
            var reservation = await _repository.GetByIds(idUtilisateur, idSession);
            if (reservation == null)
            {
                return NotFound(); // Retourne 404 si la réservation n'est pas trouvée
            }
            return Ok(reservation);
        }

        // Endpoint GET pour récupérer les réservations d'un utilisateur spécifique
        [HttpGet("idUtilisateur:{idUtilisateur}")]
        public async Task<ActionResult<IEnumerable<ReserverModel>>> RecupererReservationsParIdUtilisateur(int idUtilisateur)
        {
            var reservations = await _repository.GetByUtilisateurId(idUtilisateur);
            if (reservations == null || !reservations.Any())
            {
                return NotFound(); // Retourne 404 si aucune réservation n'est trouvée
            }
            return Ok(reservations);
        }

        // Endpoint GET pour récupérer les réservations d'une session spécifique
        [HttpGet("idSession:{idSession}")]
        public async Task<ActionResult<IEnumerable<ReserverModel>>> RecupererReservationsParIdSession(int idSession)
        {
            var reservations = await _repository.GetBySessionId(idSession);
            if (reservations == null || !reservations.Any())
            {
                return NotFound(); // Retourne 404 si aucune réservation n'est trouvée
            }
            return Ok(reservations);
        }
        #endregion

        #region Création reservation
        // Endpoint POST pour créer une nouvelle réservation
        [HttpPost]
        public async Task<ActionResult<ReserverModel>> CreationReservation([FromBody] ReserverModel reservation)
        {
            if (reservation == null)
            {
                return BadRequest("Données de réservation invalides.");
            }

            var result = await _repository.Add(reservation);
            if (result)
            {
                return Created("reservation créée", reservation); // Retourne 201 si la création réussit
            }
            return BadRequest("Erreur lors de la création de la réservation.");
        }
        #endregion

        #region Mise à jour reservation
        // Endpoint PUT pour mettre à jour une réservation existante
        [HttpPut("{idUtilisateur},{idSession}")]
        public async Task<ActionResult> MiseAJourReservation([FromBody] ReserverModel reservation, int idUtilisateur, int idSession)
        {
            var reservationExistante = await _repository.GetByIds(idUtilisateur, idSession);
            if (reservationExistante == null)
            {
                return NotFound(); // Retourne 404 si la réservation à mettre à jour n'existe pas
            }

            var aEteMisAJour = await _repository.Update(reservation, idUtilisateur, idSession);
            if (!aEteMisAJour)
            {
                return BadRequest("Erreur : Mise à jour impossible");
            }
            return Ok();
        }
        #endregion

        #region Suppression reservation
        // Endpoint DELETE pour supprimer une réservation
        [HttpDelete("{idUtilisateur},{idSession}")]
        public async Task<ActionResult> SuppressionReservation(int idUtilisateur, int idSession)
        {
            var reservationASupprimer = await _repository.GetByIds(idUtilisateur, idSession);
            if (reservationASupprimer == null)
            {
                return NotFound(); // Retourne 404 si la réservation à supprimer n'existe pas
            }

            var aEteSupprime = await _repository.Delete(idUtilisateur, idSession);
            if (!aEteSupprime)
            {
                return BadRequest("Erreur : Suppression impossible");
            }
            return Ok();
        }
        #endregion
    }
}