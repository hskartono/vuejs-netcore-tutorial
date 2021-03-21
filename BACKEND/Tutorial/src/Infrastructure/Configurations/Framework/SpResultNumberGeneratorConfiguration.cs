using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tutorial.ApplicationCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tutorial.Infrastructure.Configurations
{
	public class SpResultNumberGeneratorConfiguration : IEntityTypeConfiguration<SpResultNumberGenerator>
	{
		public void Configure(EntityTypeBuilder<SpResultNumberGenerator> builder)
		{
			builder.HasNoKey();
		}
	}
}
