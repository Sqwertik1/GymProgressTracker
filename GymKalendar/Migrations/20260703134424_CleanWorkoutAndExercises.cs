using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GymKalendar.Migrations
{
    /// <inheritdoc />
    public partial class CleanWorkoutAndExercises : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NameOfExercise",
                table: "Workouts");

            migrationBuilder.DropColumn(
                name: "Reps",
                table: "Workouts");

            migrationBuilder.DropColumn(
                name: "Weight",
                table: "Workouts");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "NameOfExercise",
                table: "Workouts",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Reps",
                table: "Workouts",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Weight",
                table: "Workouts",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }
    }
}
