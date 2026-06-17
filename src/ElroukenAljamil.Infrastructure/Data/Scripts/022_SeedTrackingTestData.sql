-- ============================================
-- 022: Seed test data for tracking (views, favorites, messages)
-- ============================================

-- Get some annonce IDs
DECLARE @id1 INT, @id2 INT, @id3 INT;
SELECT TOP 1 @id1 = Id FROM Annonces WHERE Status = 'published' ORDER BY CreatedAt DESC;
SELECT @id2 = Id FROM Annonces WHERE Status = 'published' AND Id != @id1 ORDER BY CreatedAt DESC;
SELECT @id3 = Id FROM Annonces WHERE Status = 'published' AND Id NOT IN (@id1, @id2) ORDER BY CreatedAt DESC;

-- Seed views (simulate multiple users viewing annonces)
IF @id1 IS NOT NULL
BEGIN
    INSERT INTO AnnonceViews (AnnonceId, UserId, ViewedAt) VALUES
    (@id1, 'user-101', DATEADD(DAY, -5, GETUTCDATE())),
    (@id1, 'user-102', DATEADD(DAY, -4, GETUTCDATE())),
    (@id1, 'user-103', DATEADD(DAY, -3, GETUTCDATE())),
    (@id1, 'user-104', DATEADD(DAY, -2, GETUTCDATE())),
    (@id1, 'user-105', DATEADD(DAY, -1, GETUTCDATE())),
    (@id1, 'user-106', GETUTCDATE()),
    (@id1, 'user-107', GETUTCDATE()),
    (@id1, 'user-108', GETUTCDATE()),
    (@id1, 'user-109', GETUTCDATE()),
    (@id1, 'user-110', GETUTCDATE());
END

IF @id2 IS NOT NULL
BEGIN
    INSERT INTO AnnonceViews (AnnonceId, UserId, ViewedAt) VALUES
    (@id2, 'user-201', DATEADD(DAY, -3, GETUTCDATE())),
    (@id2, 'user-202', DATEADD(DAY, -2, GETUTCDATE())),
    (@id2, 'user-203', DATEADD(DAY, -1, GETUTCDATE())),
    (@id2, 'user-204', GETUTCDATE()),
    (@id2, 'user-205', GETUTCDATE());
END

IF @id3 IS NOT NULL
BEGIN
    INSERT INTO AnnonceViews (AnnonceId, UserId, ViewedAt) VALUES
    (@id3, 'user-301', DATEADD(DAY, -1, GETUTCDATE())),
    (@id3, 'user-302', GETUTCDATE()),
    (@id3, 'user-303', GETUTCDATE());
END

-- Seed favorites
IF @id1 IS NOT NULL
BEGIN
    INSERT INTO AnnonceFavorites (AnnonceId, UserId, CreatedAt) VALUES
    (@id1, 'user-102', DATEADD(DAY, -3, GETUTCDATE())),
    (@id1, 'user-105', DATEADD(DAY, -2, GETUTCDATE())),
    (@id1, 'user-108', DATEADD(DAY, -1, GETUTCDATE()));
END

IF @id2 IS NOT NULL
BEGIN
    INSERT INTO AnnonceFavorites (AnnonceId, UserId, CreatedAt) VALUES
    (@id2, 'user-201', DATEADD(DAY, -2, GETUTCDATE())),
    (@id2, 'user-204', GETUTCDATE());
END

-- Seed messages
IF @id1 IS NOT NULL
BEGIN
    INSERT INTO Messages (AnnonceId, SenderId, SenderEmail, ReceiverId, Content, IsRead, CreatedAt) VALUES
    (@id1, 'user-102', 'ahmed@test.com', 'habib.benradhouene@gmail.com', N'Bonjour, est-ce que le vehicule est encore disponible ?', 1, DATEADD(DAY, -3, GETUTCDATE())),
    (@id1, 'user-105', 'sara@test.com', 'habib.benradhouene@gmail.com', N'Quel est le dernier prix ?', 0, DATEADD(DAY, -1, GETUTCDATE())),
    (@id1, 'user-108', 'mohamed@test.com', 'habib.benradhouene@gmail.com', N'Je suis interesse, peut-on organiser un essai ?', 0, GETUTCDATE());
END

IF @id2 IS NOT NULL
BEGIN
    INSERT INTO Messages (AnnonceId, SenderId, SenderEmail, ReceiverId, Content, IsRead, CreatedAt) VALUES
    (@id2, 'user-201', 'ali@test.com', 'habib.benradhouene@gmail.com', N'Bonjour, est-ce negociable ?', 0, GETUTCDATE());
END

PRINT 'Test data seeded: 18 views, 5 favorites, 4 messages.';
