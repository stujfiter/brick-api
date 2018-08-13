using System.Collections.Generic;
using Brick.Domain;

namespace Brick.Data 
{
    public interface PieceStore 
    {
        List<Piece> FindPieces();
        void Save(Piece piece);
    }
}