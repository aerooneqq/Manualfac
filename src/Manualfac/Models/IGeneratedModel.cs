using System.Text;

namespace Manualfac.Models;

public interface IGeneratedModel
{
  void GenerateInto(StringBuilder sb, int indent);
}