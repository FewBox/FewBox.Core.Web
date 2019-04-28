use demo;
CREATE TABLE `app` (
  `Id` char(36) NOT NULL,
  `Name` varchar(45) DEFAULT NULL,
  `CreatedBy` char(36) DEFAULT NULL,
  `ModifiedBy` char(36) DEFAULT NULL,
  `CreatedTime` datetime DEFAULT NULL,
  `ModifiedTime` datetime DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;