﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using ModernRecrut.Postulations.API.Data;

#nullable disable

namespace ModernRecrut.Postulations.API.Migrations
{
    [DbContext(typeof(PostulationContext))]
    [Migration("20231208062822_MigrationPostulationsInit")]
    partial class MigrationPostulationsInit
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "6.0.24");

            modelBuilder.Entity("ModernRecrut.Postulations.API.Models.Note", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("Contenu")
                        .HasColumnType("TEXT");

                    b.Property<Guid>("IdPostulation")
                        .HasColumnType("TEXT");

                    b.Property<string>("NomEmetteur")
                        .HasColumnType("TEXT");

                    b.Property<Guid?>("PostulationId")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("PostulationId");

                    b.ToTable("Note");
                });

            modelBuilder.Entity("ModernRecrut.Postulations.API.Models.Postulation", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("DateDisponibilite")
                        .HasColumnType("TEXT");

                    b.Property<Guid>("IdCandidat")
                        .HasColumnType("TEXT");

                    b.Property<Guid>("IdOffreEmploi")
                        .HasColumnType("TEXT");

                    b.Property<decimal>("PretentionSalariale")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Postulation", (string)null);
                });

            modelBuilder.Entity("ModernRecrut.Postulations.API.Models.Note", b =>
                {
                    b.HasOne("ModernRecrut.Postulations.API.Models.Postulation", "Postulation")
                        .WithMany("Notes")
                        .HasForeignKey("PostulationId");

                    b.Navigation("Postulation");
                });

            modelBuilder.Entity("ModernRecrut.Postulations.API.Models.Postulation", b =>
                {
                    b.Navigation("Notes");
                });
#pragma warning restore 612, 618
        }
    }
}
