using UnityEngine;

public interface IUniqueRoom{
	Coordinate start{ get; set; }
	Coordinate size{ get; set; }
	Coordinate end{ get; set; }

	bool Overlap(IUniqueRoom other);
}