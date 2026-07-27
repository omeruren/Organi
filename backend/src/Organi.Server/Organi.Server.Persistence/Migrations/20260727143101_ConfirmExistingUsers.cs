using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Organi.Server.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ConfirmExistingUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Data-only migration. Email confirmation started being enforced on checkout, reviews,
            // blog comments and vendor registration; every account created before that predates the
            // confirmation email, so grandfather them in rather than locking them out. No-op on a
            // fresh database — new sign-ups still have to confirm.
            migrationBuilder.Sql("UPDATE Users SET EmailConfirmed = 1 WHERE EmailConfirmed = 0;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Intentionally empty: the original per-user confirmation state is not recoverable, and
            // un-confirming everyone would be worse than leaving them confirmed.
        }
    }
}
