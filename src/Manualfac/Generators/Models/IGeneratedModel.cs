using System.Text;

namespace Manualfac.Generators.Models;

public interface IGeneratedModel
{
  void GenerateInto(StringBuilder sb, int indent);
}