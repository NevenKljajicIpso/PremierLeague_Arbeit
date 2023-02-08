CREATE (t2:Team {teamID: 2, teamName: "Manchester United"})-[:HAS_HOME]->(s1:Stadium {stadiumID: 1, stadiumName: "Old Trafford"})-[:IS_IN]->(c1:City {cityID: 1, cityName: "Manchester"}),
(t1:Team {teamID: 1, teamName: "Arsenal"})-[:HAS_HOME]->(s2:Stadium {stadiumID: 2, stadiumName: "Emirates"})-[:IS_IN]->(c2:City {cityID: 2, cityName: "London"})<-[:IS_IN]-(s3:Stadium {stadiumID: 3, stadiumName: "Tottenham Hotspur Stadium"})<-[:HAS_HOME]-(t3:Team {teamID: 3, teamName: "Tottenham"})<-[:LIKES]-(u1:User {userID: 1, userName: "James"}),
(u2:User {userID: 2, userName: "George"})-[:LIKES]->(t4:Team {teamID: 4, teamName: "Liverpool"})-[:HAS_HOME]->(s4:Stadium {stadiumID: 4, stadiumName: "Anfield"})-[:IS_IN]->(c3:City {cityID: 3, cityName: "Liverpool"})<-[:IS_IN]-(s7:Stadium {stadiumID: 7, stadiumName: "Goodison Park"}),
(t5:Team {teamID: 5, teamName: "Chelsea"})-[:HAS_HOME]->(s5:Stadium {stadiumID: 5, stadiumName: "Stamford Bridge"})-[:IS_IN]->(c2)<-[:IS_IN]-(s6:Stadium {stadiumID: 6, stadiumName: "Upton Park"}),
(u3:User {userID: 3, userName: "Mike"})-[:LIKES]->(s4),
(c4:City {cityID: 4, cityName: "Bristol"}),
(c5:City {cityID: 5, cityName: "Cardif"}),
(c6:City {cityID: 6, cityName: "Birmingham"}),
(u4:User {userID: 4, userName: "Alan"}),
(u5:User {userID: 5, userName: "Joe"})

// MATCH (n)
// RETURN n;
