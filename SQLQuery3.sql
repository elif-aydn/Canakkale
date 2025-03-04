CREATE TABLE Images (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Url NVARCHAR(MAX) NOT NULL, -- Resmin dosya yolu veya URL'si
    Description NVARCHAR(500),  -- Resmin açıklaması (opsiyonel)
);
