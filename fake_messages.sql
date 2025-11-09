-- Script to generate 1 million fake messages for the Messages table
-- Messages will have content from "1" to "1000000"
-- CreatedAt timestamps will be evenly distributed from 30 days ago to now

-- PostgreSQL version using generate_series
DO $$
DECLARE
    start_date TIMESTAMP := NOW() - INTERVAL '30 days';
    end_date TIMESTAMP := NOW();
    time_increment INTERVAL := (end_date - start_date) / 1000000;
    msg_timestamp TIMESTAMP;
    message_num INTEGER;
BEGIN
    FOR message_num IN 1..1000000 LOOP
        msg_timestamp := start_date + (time_increment * message_num);
        
        INSERT INTO "Messages" (
            "Id",
            "Content",
            "MessageType",
            "FileUrl",
            "FileName",
            "FileType",
            "FileSize",
            "SenderId",
            "ConversationId",
            "GroupId",
            "IsRead",
            "CreatedAt",
            "UpdatedAt",
            "UpdatedBy",
            "CreatedBy"
        ) VALUES (
            gen_random_uuid(),
            message_num::TEXT,
            0, -- MessageTypes.Text
            NULL,
            NULL,
            NULL,
            NULL,
            '0e6fb8f1-3e7c-4f7b-9e1d-72b85919c1cb', -- SenderId from user 10
            NULL,
            'a00f4de7-3d98-458d-8dd5-69ad46e5b046', -- GroupId
            false,
            msg_timestamp,
            NULL,
            NULL,
            NULL
        );
        
        -- Commit in batches of 10,000 to avoid memory issues
        IF message_num % 10000 = 0 THEN
            RAISE NOTICE 'Inserted % messages...', message_num;
        END IF;
    END LOOP;
    
    RAISE NOTICE 'Successfully inserted 1,000,000 messages!';
END $$;

-- Alternative approach using generate_series (faster for PostgreSQL)
-- Uncomment this and comment out the above DO block if you prefer this method

/*
INSERT INTO "Messages" (
    "Id",
    "Content",
    "MessageType",
    "FileUrl",
    "FileName",
    "FileType",
    "FileSize",
    "SenderId",
    "ConversationId",
    "GroupId",
    "IsRead",
    "CreatedAt",
    "UpdatedAt",
    "UpdatedBy",
    "CreatedBy"
)
SELECT 
    gen_random_uuid() AS "Id",
    n::TEXT AS "Content",
    0 AS "MessageType", -- MessageTypes.Text
    NULL AS "FileUrl",
    NULL AS "FileName",
    NULL AS "FileType",
    NULL AS "FileSize",
    '0e6fb8f1-3e7c-4f7b-9e1d-72b85919c1cb' AS "SenderId",
    NULL AS "ConversationId",
    'a00f4de7-3d98-458d-8dd5-69ad46e5b046' AS "GroupId",
    false AS "IsRead",
    (NOW() - INTERVAL '30 days') + ((NOW() - (NOW() - INTERVAL '30 days')) / 1000000 * n) AS "CreatedAt",
    NULL AS "UpdatedAt",
    NULL AS "UpdatedBy",
    NULL AS "CreatedBy"
FROM generate_series(1, 1000000) AS n;
*/
