using QuickGraph;
/// <summary>
/// Interaction logic for HexEdge
/// This class was created with Alt+Enter shortcut as a missing part of
/// BidirectionalGraph<Hex,HexEdge> myHexGraph;
/// While 1 the 2 required generic types Hex, was already made, the other part
/// is this very class. It has to implement the IEdge<Hex> interface
/// and define 2 items as seen down below...
/// Source and Target seems to be the 2 items that should be navigateable in either direction
/// 
/// I have no idea what to write in place of the exceptions...
/// </summary>
namespace Hexagonites
{
    internal class HexEdge : IEdge<Hex>
    {
        public Hex Source => throw new System.NotImplementedException(); //What is dis..??

        public Hex Target => throw new System.NotImplementedException();
    }
}