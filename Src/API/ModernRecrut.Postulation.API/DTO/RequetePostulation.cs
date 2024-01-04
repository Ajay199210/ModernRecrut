﻿using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ModernRecrut.Postulations.API.DTO
{
    public class RequetePostulation
    {
        [Required, Display(Name = "Id candidat")]
        public Guid IdCandidat { get; set; }

        [Required, Display(Name = "Id offre emploi")]
        public Guid IdOffreEmploi { get; set; }

        [
            Required(ErrorMessage = "Veuillez svp renseigner vos prétentions salariales"),
            DisplayName("Prétention salariales")
        ]
        public decimal PretentionSalariale { get; set; }
        
        [
		    Required(ErrorMessage = "Veuillez svp renseigner la date de diponibilité pour l'entrevue"),
            DisplayName("Prétention salariale"), 
		    DataType(DataType.Date),
		    DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)
        ]
        public DateTime DateDisponibilite { get; set; }
    }
}
