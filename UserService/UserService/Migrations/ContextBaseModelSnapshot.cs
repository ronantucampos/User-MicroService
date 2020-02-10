using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using UserService.Entities.Base;

namespace UserService.Migrations
{
    [DbContext(typeof(ContextBase))]
    partial class ContextBaseModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.2")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("UserService.Entities.Usuario", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<long?>("AlteradoPor");

                    b.Property<long>("CriadoPor");

                    b.Property<DateTime?>("DataAlteracao");

                    b.Property<DateTime>("DataCriacao");

                    b.Property<DateTime?>("DataExclusao");

                    b.Property<string>("Email");

                    b.Property<long?>("ExcluidoPor");

                    b.Property<string>("Login");

                    b.Property<string>("Nome");

                    b.Property<string>("Senha");

                    b.Property<string>("Token");

                    b.Property<DateTime?>("ValidadeToken");

                    b.HasKey("Id");

                    b.ToTable("Usuarios");
                });
        }
    }
}
